import { useEffect } from "react";
import { useTodoGroupStore } from "./todoGroupStore";
import { Button, Form, Input, notification, Space } from "antd";
import { TodoGroupService } from "./todoGroupService";

export const TodoGroupEdit = () => {
  const { editTodoGroup, setTodoGroupEdit } = useTodoGroupStore();
  const [form] = Form.useForm();

  useEffect(() => {
    if (editTodoGroup?.id) {
      form.setFieldsValue(editTodoGroup);
    }
  }, [editTodoGroup, form]);

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      values.id = editTodoGroup?.id;

      const data = await TodoGroupService.updateTodoGroup(values);
      console.log("Todo group updated:", data);
      notification.success({ message: "Operation successful!" });
      setTodoGroupEdit(null); // Reset edit mode
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

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button type="default" onClick={() => setTodoGroupEdit(null)}>
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </div>
  );
};
