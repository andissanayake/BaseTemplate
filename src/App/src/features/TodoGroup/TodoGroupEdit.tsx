import React, { useEffect } from "react";
import {
  Form,
  Input,
  notification,
  Select,
  Button,
  Space,
  Typography,
} from "antd";
import { useTodoGroupStore } from "./todoGroupStore";
import { useNavigate, useParams } from "react-router-dom";
import { TodoGroupService } from "./todoGroupService";

const TodoGroupEdit: React.FC = () => {
  const { currentTodoGroup, setTodoGroupCurrent, updateTodoGroup } =
    useTodoGroupStore();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { listId } = useParams();
  if (!listId) throw new Error("listId is required");

  useEffect(() => {
    form.setFieldsValue(currentTodoGroup);
  }, [currentTodoGroup, form]);

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      values.id = currentTodoGroup?.id;

      try {
        await updateTodoGroup(values);
        notification.success({ message: "Todo list updated successfully!" });
        setTodoGroupCurrent(null); // Clear current todo group from store
        navigate("/todo-list");
      } catch (error) {
        console.error("Error updating todo list:", error);
        notification.error({ message: "Failed to update todo list!" });
      }
    });
  };

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo List Edit
        </Typography.Title>
      </Space>
      <Form form={form} layout="vertical" onFinish={handleSaveTodoGroup}>
        <Form.Item
          label="Todo list Name"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo list name!" },
          ]}
        >
          <Input placeholder="Enter todo list name" />
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
    </>
  );
};

export default TodoGroupEdit;
