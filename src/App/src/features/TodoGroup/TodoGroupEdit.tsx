/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect } from "react";
import { useTodoGroupStore } from "./todoGroupStore";
import { Button, Form, Input, notification, Space, Select } from "antd";
import { TodoGroupService } from "./todoGroupService";
import { useNavigate, useParams } from "react-router-dom";

export const TodoGroupEdit = () => {
  const { currentTodoGroup, setTodoGroupCurrent } = useTodoGroupStore();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { id } = useParams();

  useEffect(() => {
    if (id) {
      TodoGroupService.fetchTodoGroupById(id).then((res) =>
        setTodoGroupCurrent(res.data, null)
      );
    }
  }, [id, setTodoGroupCurrent]);

  useEffect(() => {
    if (currentTodoGroup?.id) {
      form.setFieldsValue(currentTodoGroup);
    }
  }, [currentTodoGroup, form]);

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      values.id = currentTodoGroup?.id;

      try {
        await TodoGroupService.updateTodoGroup(values);
        notification.success({ message: "Operation successful!" });
        setTodoGroupCurrent(null, null);
        navigate("/todo-list");
      } catch (error: any) {
        console.error("Error updating todo group:", error);
        notification.error({ message: "Failed to update todo group!" });
      }
    });
  };

  return (
    <div>
      <Form form={form} layout="vertical" onFinish={handleSaveTodoGroup}>
        <Form.Item
          label="Todo Group Name"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo group name!" },
          ]}
        >
          <Input placeholder="Enter todo group name" />
        </Form.Item>

        <Form.Item
          label="Select Colour"
          name="colour"
          rules={[{ required: true, message: "Please select a colour!" }]}
        >
          <Select optionLabelProp="label">
            {TodoGroupService.getColours().map((colour) => (
              <Select.Option
                key={colour.value}
                value={colour.value}
                label={
                  <span style={{ display: "flex", alignItems: "center" }}>
                    <span
                      style={{
                        display: "inline-block",
                        width: 20,
                        height: 20,
                        backgroundColor: colour.value,
                        marginRight: 10,
                        borderRadius: "50%",
                      }}
                    />
                    {colour.label}
                  </span>
                }
              >
                <span style={{ display: "flex", alignItems: "center" }}>
                  <span
                    style={{
                      display: "inline-block",
                      width: 20,
                      height: 20,
                      backgroundColor: colour.value,
                      marginRight: 10,
                      borderRadius: "50%",
                    }}
                  />
                  {colour.label}
                </span>
              </Select.Option>
            ))}
          </Select>
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button type="default" onClick={() => navigate("/todo-list")}>
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </div>
  );
};
